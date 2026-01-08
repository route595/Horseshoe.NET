using System;
using System.Collections.Generic;
using System.Linq;

using Horseshoe.NET.DateAndTime;
using Horseshoe.NET.ObjectsTypesAndValues;
using static Horseshoe.NET.Guardrails;

namespace Horseshoe.NET.Mathematics.Finance.Budget
{
    public class Budget : List<BudgetItem>
    {
        public DateTime StartDate { get; set; } = MinDate;
        public DateTime EndDate { get; set; } = MaxDate;
        public Account Account { get; set; }
        public decimal StartingAmount { get; set; }
        public BudgetItemSort ItemSort { get; set; }

        public void Validate()
        {
            if (StartDate < MinDate || StartDate > MaxDate)
                throw new ArgumentException("Budget start date cannot be outside the range Guardrails.MinDate to Guardrails.MaxDate.");
            if (EndDate < MinDate || EndDate > MaxDate)
                throw new ArgumentException("Budget end date cannot be outside the range Guardrails.MinDate to Guardrails.MaxDate.");
            if (StartDate > EndDate)
                throw new ArgumentException("Budget end date cannot be later than end date.");
        }

        public void SortItems()
        {
            Sort(ItemSort);
        }

        /// <summary>
        /// This method executes a sequence of steps to build and return a list of <see cref="Budget"/> 
        /// instances from the supplied collection of type <see cref="BudgetPlanningItem"/>.
        /// The caller can affect the build process by supplying the desired item ordering
        /// as well as interact with individual <see cref="Budget"/> instances at any of three different
        /// phases of the build process.
        /// </summary>
        /// <param name="budgetPlanningItems">A collection of <see cref="BudgetPlanningItem"/> such single and recurring expenses, etc.</param>
        /// <param name="from">The earliest transaction date any generated <see cref="BudgetItem"/>.</param>
        /// <param name="to">The latest transaction date any generated <see cref="BudgetItem"/>.</param>
        /// <param name="itemOrdering">An item ordering hint to be considered during the ordering phase of the build process.</param>
        /// <param name="prePopulate">Allows the caller to interact with each involved <see cref="Account"/>'s <see cref="Budget"/> prior to step 1 of 1) populating the <see cref="BudgetItem"/>s, 2) sorting them, 3) applying disbursements and 4) calculating the running total.</param>
        /// <param name="preSort">Allows the caller to interact with each involved <see cref="Account"/>'s <see cref="Budget"/> prior to step 2 of 1) populating the <see cref="BudgetItem"/>s, 2) sorting them, 3) applying disbursements and 4) calculating the running total.</param>
        /// <param name="preDisburse">Allows the caller to interact with each involved <see cref="Account"/>'s <see cref="Budget"/> prior to step 3 of 1) populating the <see cref="BudgetItem"/>s, 2) sorting them, 3) applying disbursements and 4) calculating the running total.</param>
        /// <param name="preCalc">Allows the caller to interact with each involved <see cref="Account"/>'s <see cref="Budget"/> prior to step 4 of 1) populating the <see cref="BudgetItem"/>s, 2) sorting them, 3) applying disbursements and 4) calculating the running total.</param>
        /// <returns></returns>
        public static List<Budget> GenerateFromPlanningItems
        (
            IEnumerable<IBudgetPlanningItem> budgetPlanningItems, 
            DateTime? from = null, 
            DateTime? to = null,
            BudgetItemOrdering itemOrdering = default, 
            Action<Budget> prePopulate = null,
            Action<Budget> preSort = null,
            Action<Budget> preDisburse = null,
            Action<Budget> preCalc = null
        )
        {
            var tempBudget = new Budget
            {
                StartDate = Wrangle(from ?? DateUtil.MonthStart()),
                EndDate = Wrangle(to ?? DateUtil.MonthEnd()),
                ItemSort = new BudgetItemSort(itemOrdering)
            };

            tempBudget.Validate();

            foreach (var item in budgetPlanningItems)
            {
                ValidateBudgetPlanningItemType(item);

                if (item is ITransferPlanningItem transferItem)
                {
                    if (item is IOneTimeBudgetPlanningItem oneTimeItem && oneTimeItem.TransactionDate >= tempBudget.StartDate && oneTimeItem.TransactionDate <= tempBudget.EndDate)
                    {
                        var source = new OneTimeExpenseBudgetPlanningItem
                        {
                            ParentBudgetPlanningItem = item,
                            Name = item.Name,
                            Amount = item.Amount,
                            Account = transferItem.SourceAccount,
                            TransactionDate = oneTimeItem.TransactionDate,
                            //Category = "Bank Transfer"
                        };
                        var destination = new OneTimeIncomeBudgetPlanningItem
                        {
                            ParentBudgetPlanningItem = item,
                            Name = item.Name,
                            Amount = item.Amount,
                            Account = item.Account,
                            TransactionDate = oneTimeItem.TransactionDate,
                            //Category = "Bank Transfer"
                        };
                        tempBudget.Add(new BudgetItem(source as IBudgetPlanningItem));
                        tempBudget.Add(new BudgetItem(destination));
                    }

                    if (item is IRecurringBudgetPlanningItem recurringItem)
                    {
                        var dates = recurringItem.Recurrence.GetAllOccurrances(tempBudget.StartDate, tempBudget.EndDate);
                        foreach (var date in dates)
                        {
                            var source = new OneTimeExpenseBudgetPlanningItem
                            {
                                ParentBudgetPlanningItem = item,
                                Name = item.Name,
                                Amount = item.Amount,
                                Account = transferItem.SourceAccount,
                                TransactionDate = date,
                                //Category = "Bank Transfer"
                            };
                            var destination = new OneTimeIncomeBudgetPlanningItem
                            {
                                ParentBudgetPlanningItem = item,
                                Name = item.Name,
                                Amount = item.Amount,
                                Account = item.Account,
                                TransactionDate = date,
                                //Category = "Bank Transfer"
                            };
                            tempBudget.Add(new BudgetItem(source));
                            tempBudget.Add(new BudgetItem(destination));
                        }
                    }
                }
                else if (item is IExpensePlanningItem)
                {
                    if (item is IOneTimeBudgetPlanningItem oneTimeItem && oneTimeItem.TransactionDate >= tempBudget.StartDate && oneTimeItem.TransactionDate <= tempBudget.EndDate)
                    {
                        tempBudget.Add(new BudgetItem(item));
                    }

                    if (item is IRecurringBudgetPlanningItem recurringItem)
                    {
                        var dates = recurringItem.Recurrence.GetAllOccurrances(tempBudget.StartDate, tempBudget.EndDate);
                        foreach (var date in dates)
                        {
                            tempBudget.Add(new BudgetItem(item) { TransactionDate = date });
                        }
                    }
                }
                else if (item is IIncomePlanningItem incomeItem)
                {
                    DateTime transactionDate;
                    if (item is IOneTimeBudgetPlanningItem oneTimeItem && oneTimeItem.TransactionDate >= tempBudget.StartDate && oneTimeItem.TransactionDate <= tempBudget.EndDate)
                    {
                        tempBudget.Add(new BudgetItem(item));
                        transactionDate = oneTimeItem.TransactionDate;
                        autoDisburse();
                        if (incomeItem.AutoDisbursements != null)
                        {
                            foreach (var disbursement in incomeItem.AutoDisbursements)
                            {
                                var disbursementItem = new BudgetItem(item)
                                {
                                    Name = disbursement.Name ?? $"Auto Disbursement from {item.Name}",
                                    Amount = disbursement.Calc(item.Amount) * -1m,
                                    Account = item.Account,
                                    TransactionDate = oneTimeItem.TransactionDate,
                                    Category = disbursement.Category?.ToString()
                                };
                                tempBudget.Add(disbursementItem);
                                if (disbursement.DestinationAccount != null)
                                {
                                    disbursementItem = new BudgetItem(item)
                                    {
                                        Name = disbursement.Name ?? $"Auto Disbursement from {item.Name}",
                                        Amount = disbursement.Calc(item.Amount),
                                        Account = disbursement.DestinationAccount,
                                        TransactionDate = oneTimeItem.TransactionDate,
                                        Category = disbursement.Category?.ToString()
                                    };
                                    tempBudget.Add(disbursementItem);
                                }
                            }
                        }
                    }

                    if (item is IRecurringBudgetPlanningItem recurringItem)
                    {
                        var dates = recurringItem.Recurrence.GetAllOccurrances(tempBudget.StartDate, tempBudget.EndDate);
                        foreach (var date in dates)
                        {
                            tempBudget.Add(new BudgetItem(item) { TransactionDate = transactionDate = date });
                            autoDisburse();
                        }
                    }

                    void autoDisburse()  // dest account only, current acct disbursements happen in steps 1 - 4
                    {
                        if (incomeItem.AutoDisbursements != null)
                        {
                            foreach (var disbursement in incomeItem.AutoDisbursements)
                            {
                                //var disbursementItem = new BudgetItem(item)
                                //{
                                //    Name = disbursement.Name ?? $"Auto Disbursement from {item.Name}",
                                //    Amount = disbursement.Calc(item.Amount) * -1m,
                                //    Account = item.Account,
                                //    TransactionDate = transactionDate,
                                //    Category = disbursement.Category?.ToString()
                                //};
                                //tempBudget.Add(disbursementItem);
                                if (disbursement.DestinationAccount != null)
                                {
                                    var disbursementItem = new BudgetItem(item)
                                    {
                                        Name = disbursement.Name ?? $"Auto Disbursement from {item.Name}",
                                        Amount = disbursement.Calc(item.Amount),
                                        Account = disbursement.DestinationAccount,
                                        TransactionDate = transactionDate,
                                        Category = disbursement.Category?.ToString()
                                    };
                                    tempBudget.Add(disbursementItem);
                                }
                            }
                        }
                    }
                }
            }

            // break up items by Account
            var budgetGroups = tempBudget.GroupBy(budgetItem => budgetItem.Account);
            var list = new List<Budget>();
            foreach (var group in budgetGroups)
            {
                var budget = new Budget 
                { 
                    Account = group.Key, 
                    StartDate = tempBudget.StartDate, 
                    EndDate = tempBudget.EndDate, 
                    ItemSort = tempBudget.ItemSort 
                };

                prePopulate?.Invoke(budget);

                budget.AddRange(group);                         // step 1

                preSort?.Invoke(budget);

                budget.SortItems();                             // step 2

                preDisburse?.Invoke(budget);

                for (int i = budget.Count - 1; i >= 0; i--)     // step 3
                {
                    if (budget[i].BudgetPlanningItem is IIncomePlanningItem incomeItem && incomeItem.HasAutoDisbursements)
                    {
                        foreach (var disbursement in incomeItem.AutoDisbursements)
                        {
                            var disbursementItem = new BudgetItem(budget[i].BudgetPlanningItem)
                            {
                                Name = disbursement.Name ?? $"Auto Disbursement from {budget[i].Name}",
                                Amount = disbursement.Calc(budget[i].Amount) * -1m,
                                Account = budget[i].Account,
                                TransactionDate = budget[i].TransactionDate,
                                Category = disbursement.Category?.ToString()
                            };
                            budget.Insert(i + 1, disbursementItem);
                        }
                    }
                }

                preCalc?.Invoke(budget);

                var startingAmount = budget.StartingAmount;
                foreach (var item in budget)                    // step 4
                {
                    item.RunningTotal = startingAmount += item.Amount;
                }

                list.Add(budget);
            }

            return list;
        }

        public static IBudgetPlanningItem CreateIncomePlanningItem(string name, decimal amount, DateTime transactionDate, Account account = null, IEnumerable<AutoDisbursement> autoDisbursements = null)
        {
            return new OneTimeIncomeBudgetPlanningItem
            {
                Name = name,
                Amount = amount,
                TransactionDate = transactionDate,
                Account = account
            }.AddAutoDisbursements(autoDisbursements);
        }

        public static IBudgetPlanningItem CreateIncomePlanningItem(string name, decimal amount, Recurrence recurrence, Account account = null, IEnumerable<AutoDisbursement> autoDisbursements = null)
        {
            return new RecurringIncomeBudgetPlanningItem
            {
                Name = name,
                Amount = amount,
                Recurrence = recurrence,
                Account = account
            }.AddAutoDisbursements(autoDisbursements);
        }

        public static IBudgetPlanningItem CreateExpensePlanningItem(string name, decimal amount, DateTime transactionDate, Account account = null, ExpenseCategory category = default)
        {
            return new OneTimeExpenseBudgetPlanningItem
            {
                Name = name,
                Amount = amount,
                TransactionDate = transactionDate,
                Account = account,
                Category = category
            };
        }

        public static IBudgetPlanningItem CreateExpensePlanningItem(string name, decimal amount, Recurrence recurrence, Account account = null, ExpenseCategory category = default)
        {
            return new RecurringExpenseBudgetPlanningItem
            {
                Name = name,
                Amount = amount,
                Recurrence = recurrence,
                Account = account,
                Category = category
            };
        }

        public static IBudgetPlanningItem CreateTransferPlanningItem(string name, decimal amount, DateTime transactionDate, Account account = null, Account sourceAccount = null)
        {
            return new OneTimeTransferBudgetPlanningItem
            {
                Name = name,
                Amount = amount,
                TransactionDate = transactionDate,
                Account = account,
                SourceAccount = sourceAccount
            };
        }

        public static IBudgetPlanningItem CreateTransferPlanningItem(string name, decimal amount, Recurrence recurrence, Account account = null, Account sourceAccount = null)
        {
            return new RecurringTransferBudgetPlanningItem
            {
                Name = name,
                Amount = amount,
                Recurrence = recurrence,
                Account = account,
                SourceAccount = sourceAccount
            };
        }

        private static IList<Type> _budgetPlanningItemTypeCache = new List<Type>();

        private static void ValidateBudgetPlanningItemType(IBudgetPlanningItem item)
        {
            if (_budgetPlanningItemTypeCache.Contains(item.GetType()))
                return;

            if (item is IExpensePlanningItem)
            {
                if (item is IIncomePlanningItem)
                    throw new TypeException("IBudgetPlanningItem types inheriting from IExpensePlanningItem cannot also inherit from IIncomePlanningItem.");
                if (item is ITransferPlanningItem)
                    throw new TypeException("IBudgetPlanningItem types inheriting from IExpensePlanningItem cannot also inherit from ITransferPlanningItem.");
            }
            else if (item is IIncomePlanningItem)
            {
                if (item is ITransferPlanningItem)
                    throw new TypeException("IBudgetPlanningItem types inheriting from IIncomePlanningItem cannot also inherit from ITransferPlanningItem.");
            }
            else if (!(item is ITransferPlanningItem))
                throw new TypeException("IBudgetPlanningItem types must also inherit from IExpensePlanningItem, IIncomePlanningItem or ITransferPlanningItem.");

            if (item is IOneTimeBudgetPlanningItem)
            {
                if (item is IRecurringBudgetPlanningItem)
                    throw new TypeException("IBudgetPlanningItem types inheriting from IOneTimeBudgetPlanningItem cannot also inherit from IRecurringBudgetPlanningItem.");
            }
            else if (!(item is IRecurringBudgetPlanningItem))
                throw new TypeException("IBudgetPlanningItem types must also inherit from IOneTimeBudgetPlanningItem or IRecurringBudgetPlanningItem.");

            _budgetPlanningItemTypeCache.Add(item.GetType());
        }
    }
}
