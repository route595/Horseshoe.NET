using System;
using System.Collections.Generic;
using System.Linq;

namespace Horseshoe.NET.Finance
{
    public static class FinanceEngine
    {
        /// <summary>
        /// I=PRT
        /// </summary>
        /// <param name="presentValue">Principal.</param>
        /// <param name="annualRate">APY.</param>
        /// <param name="numberPeriods">Number of days, months, quarters, etc.</param>
        /// <param name="compoundingPeriod">Periods measured in days, months, quarters, etc.</param>
        /// <param name="suppressRounding">If <c>true</c>, will not round results to culture-specific number of decimal places, default is <c>false</c>.</param>
        /// <returns>The amount of non-compounded interest given the principal, rate and time.</returns>
        public static decimal CalculateSimpleInterest(decimal presentValue, decimal annualRate, int numberPeriods, CompoundingPeriod compoundingPeriod = CompoundingPeriod.Monthly, bool suppressRounding = false, int? convertedAnnualRateDecimalPoints = null, Action<decimal> onAnnualRateConverted = null)
        {
            decimal value = presentValue * ConvertAnnualRate(annualRate, compoundingPeriod: compoundingPeriod, convertedAnnualRateDecimalPoints: convertedAnnualRateDecimalPoints, onAnnualRateConverted: onAnnualRateConverted) * numberPeriods;
            return suppressRounding
                ? value
                : Math.Round(value, Settings.CurrencyDecimalDigits);
        }

        public static decimal CalculateCompoundInterest(decimal presentValue, decimal annualRate, int numberCompoundingPeriods, int years, CompoundingPeriod compoundingPeriod = CompoundingPeriod.Monthly, bool suppressRounding = false, int? convertedAnnualRateDecimalPoints = null)
        {
            var value = (decimal)((double)presentValue * Math.Pow(1D + (double)annualRate / numberCompoundingPeriods, numberCompoundingPeriods * years));
            return suppressRounding
                ? value
                : Math.Round(value, Settings.CurrencyDecimalDigits);
        }

        public static decimal CalculateFutureValueFromPresentValue(decimal presentValue, decimal annualRate, int numberCompoundingPeriods, CompoundingPeriod compoundingPeriod = CompoundingPeriod.Monthly, bool suppressRounding = false, int? convertedAnnualRateDecimalPoints = null)
        {
            var value = presentValue * (decimal)Math.Pow(1D + (double)annualRate, numberCompoundingPeriods);
            return suppressRounding
               ? value
               : Math.Round(value, Settings.CurrencyDecimalDigits);
        }

        public static decimal CalculateFutureValueFromPaymentAmount(decimal paymentAmount, decimal annualRate, int numberPayments, CompoundingPeriod compoundingPeriod = CompoundingPeriod.Monthly, bool suppressRounding = false, int? convertedAnnualRateDecimalPoints = null)
        {
            var value = paymentAmount * (decimal)((Math.Pow(1 + (double)annualRate, numberPayments) / (double)annualRate) - (1D / (double)annualRate));
            return suppressRounding
                ? value
                : Math.Round(value, Settings.CurrencyDecimalDigits);
        }

        public static decimal CalculatePresentValue(decimal futureValue, decimal annualInterestRate, int numberCompoundingPeriods, int years, CompoundingPeriod compoundingPeriod = CompoundingPeriod.Monthly, bool suppressRounding = false, int? convertedAnnualRateDecimalPoints = null)
        {
            var value = futureValue / (decimal)Math.Pow(1D + (double)annualInterestRate / numberCompoundingPeriods, numberCompoundingPeriods * years);
            return suppressRounding
               ? value
               : Math.Round(value, Settings.CurrencyDecimalDigits);
        }

        public static decimal CalculateMonthlyPayment(decimal presentValue, decimal annualRate, int numberCompoundingPeriods, int numberPaymentsPerPeriod, CompoundingPeriod compoundingPeriod = CompoundingPeriod.Monthly, bool suppressRounding = false, int? convertedAnnualRateDecimalPoints = null)
        {
            var value = (decimal)
            (
                (double)(presentValue * annualRate / numberPaymentsPerPeriod) /
                (1D - Math.Pow((double)(1 + annualRate / numberPaymentsPerPeriod), -1 * numberPaymentsPerPeriod * numberCompoundingPeriods))
            );
            return suppressRounding
                ? value
                : Math.Round(value, Settings.CurrencyDecimalDigits);
        }

        public static decimal ConvertAnnualRate(decimal annualRate, CompoundingPeriod compoundingPeriod = CompoundingPeriod.Monthly, int? convertedAnnualRateDecimalPoints = null, Action<decimal> onAnnualRateConverted = null)
        {
            decimal convertedRate = annualRate;
            switch (compoundingPeriod)
            {
                case CompoundingPeriod.Daily:
                    convertedRate = annualRate / 365m;
                    break;
                case CompoundingPeriod.Weekly:
                    convertedRate = annualRate / 52m;
                    break;
                case CompoundingPeriod.Monthly:
                    convertedRate = annualRate / 12m;
                    break;
                case CompoundingPeriod.Quarterly:
                    convertedRate = annualRate / 4m;
                    break;
                case CompoundingPeriod.Yearly:
                    break;
                default:
                    throw new ValidationException("invalid or unspecified compounding period");
            }

            if (convertedAnnualRateDecimalPoints.HasValue)
                convertedRate = Math.Round(convertedRate, convertedAnnualRateDecimalPoints.Value);

            onAnnualRateConverted?.Invoke(convertedRate);
            return convertedRate;
        }

        /// <summary>
        /// Creates a <c>CreditPayoffProjection</c> complete with monthly payment calculations, payoffs, etc.
        /// </summary>
        /// <param name="creditAccounts">A list or array of <c>CreditAccount</c>s.</param>
        /// <param name="startDate">An optional <c>DateTime</c> at which to start the debt payoff calculation.</param>
        /// <param name="snowballing">If <c>true</c>, the calculated payments will snowball as account payoffs are projected.  Default is <c>false</c>.</param>
        /// <param name="extraSnowballAmount">An optional extra monthly amount to add to the payoff calculation.</param>
        /// <param name="sortOrder">How the account list was sorted (see <c>FinanceEngine.GenerateCreditPayoffProjection()</c>)</param>
        /// <returns>A fully fleshed out <c>CreditPayoffProjection</c>.</returns>
        /// <exception cref="FinanceException"></exception>
        /// <exception cref="ValidationException"></exception>
        public static DebtPayoffProjection GenerateCreditPayoffProjection(IEnumerable<CreditAccount> creditAccounts, DateTime? startDate = null, bool snowballing = false, decimal extraSnowballAmount = 0m, CreditAccountSortOrder sortOrder = CreditAccountSortOrder.NotSorted)
        {
            // sort the credit accounts
            if (creditAccounts != null)
            {
                switch (sortOrder)
                {
                    case CreditAccountSortOrder.APR:
                        creditAccounts = creditAccounts.OrderBy(ca => ca.APR);
                        break;
                    case CreditAccountSortOrder.APR_Descending:
                        creditAccounts = creditAccounts.OrderByDescending(ca => ca.APR);
                        break;
                    case CreditAccountSortOrder.Balance:
                        creditAccounts = creditAccounts.OrderBy(ca => ca.Balance);
                        break;
                    case CreditAccountSortOrder.Balance_Descending:
                        creditAccounts = creditAccounts.OrderByDescending(ca => ca.Balance);
                        break;
                }
            }

            // instantiate the payoff projections
            var projection = new DebtPayoffProjection(creditAccounts, startDate: startDate, snowballing: snowballing, extraSnowballAmount: extraSnowballAmount, sortOrder: sortOrder);
            var runningDate = projection.StartDate;

            // validation
            if (!projection.Any())
                return projection;

            while (projection.TotalRunningBalance > 0m)
            {
                projection.Dates.Add(runningDate);
                if (!snowballing)
                {
                    // pass 1 of 1 - minimum payments
                    foreach (var capi in projection)
                    {
                        if (capi.RunningBalance > 0m)
                        {
                            var currentCycleInterestAmt = capi.CalculateCurrentCycleInterest(runningDate);
                            var currentCyclePaymentAmt = capi.CalculateCurrentCyclePayment(runningDate);

                            // validation
                            if (!(currentCyclePaymentAmt > currentCycleInterestAmt))
                                throw new FinanceException($"{capi.Account.Name}: Please increase the minimum payment amount to exceed the monthly interest amount: {currentCyclePaymentAmt} < {currentCycleInterestAmt}.");

                            var monthlyPayment = new MonthlyPaymentInfo { InterestAmount = currentCycleInterestAmt };

                            // scenario 1 - account paid down
                            if (capi.RunningBalance > currentCyclePaymentAmt - monthlyPayment.InterestAmount)
                            {
                                monthlyPayment.PaymentAmount = currentCyclePaymentAmt;
                                monthlyPayment.RunningBalance = capi.RunningBalance - monthlyPayment.PaymentAmount + monthlyPayment.InterestAmount;

                            }
                            // scenario 2 - account paid off
                            else
                            {
                                monthlyPayment.PaymentAmount = capi.RunningBalance;
                                monthlyPayment.RunningBalance = 0m;
                            }
                            capi.Add(monthlyPayment);
                        }
                    }
                }
                else
                {
                    // perform monthly payment calculations
                    var remainingMonthlyBudget = projection.TotalMonthlyBudget;

                    // pass 1 of 2 - minimum payments
                    foreach (var capi in projection)
                    {
                        if (capi.RunningBalance > 0m)
                        {
                            var currentCycleInterestAmt = capi.CalculateCurrentCycleInterest(runningDate);
                            var currentCyclePaymentAmt = capi.CalculateCurrentCyclePayment(runningDate);

                            // validation
                            if (!(currentCyclePaymentAmt > capi.CalculateCurrentCycleInterest(runningDate)))
                                throw new FinanceException($"{capi.Account.Name}: Please increase the minimum payment amount to exceed the monthly interest amount: {currentCyclePaymentAmt} < {currentCycleInterestAmt}.");

                            var monthlyPayment = new MonthlyPaymentInfo { InterestAmount = currentCycleInterestAmt };

                            // scenario 1 - account paid down
                            if (capi.RunningBalance > currentCyclePaymentAmt - monthlyPayment.InterestAmount)
                            {
                                monthlyPayment.PaymentAmount = currentCyclePaymentAmt;
                                monthlyPayment.RunningBalance = capi.RunningBalance - monthlyPayment.PaymentAmount + monthlyPayment.InterestAmount;
                            }
                            // scenario 2 - account paid off
                            else
                            {
                                monthlyPayment.PaymentAmount = capi.RunningBalance + monthlyPayment.InterestAmount;
                                monthlyPayment.RunningBalance = 0m;
                            }
                            capi.Add(monthlyPayment);
                            remainingMonthlyBudget -= monthlyPayment.PaymentAmount;
                        }
                    }

                    // pass 2 of 2 - snowball payments - withSnowballing in order of original sort
                    foreach (var capi in projection)
                    {
                        var _runningBalance = capi.RunningBalance;  // eq sa.Last().RunningBalance
                        if (_runningBalance > 0m)
                        {
                            var last = capi.Last();

                            // scenario 1 - account paid down
                            if (_runningBalance > remainingMonthlyBudget)
                            {
                                last.PaymentAmount += remainingMonthlyBudget;   // increase the monthly payment
                                last.RunningBalance -= remainingMonthlyBudget;  // and adjust the running balance
                                break;                                          // exit (move on to next month)
                            }

                            // scenario 2 - account paid off
                            else
                            {
                                last.PaymentAmount += _runningBalance;
                                last.RunningBalance = 0m;
                                if (_runningBalance == remainingMonthlyBudget)  // if remainingMonthlyBudget was exhausted
                                    break;                                      // exit (move on to next month)
                                remainingMonthlyBudget -= _runningBalance;      // otherwise, reduce remainingMonthlyBudget and continue
                            }
                        }
                    }
                }

                // increment date
                runningDate = runningDate.AddMonths(1);
            }
            return projection;
        }
    }
}
