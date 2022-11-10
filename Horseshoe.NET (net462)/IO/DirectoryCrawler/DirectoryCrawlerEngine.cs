using System;

namespace Horseshoe.NET.IO.DirectoryCrawler
{
    public class DirectoryCrawler : DirectoryCrawler<DirectoryPath>
    {
        public DirectoryCrawler
        (
            DirectoryPath root,
            DirectoryCrawledEvent<DirectoryPath> directoryCrawled = null,
            CrawlOptions options = null
        ) : base
        (
            root,
            directoryCrawled,
            options
        )
        {
        }

        public override DirectoryPath CrawlComplete()
        {
            return Root;
        }
    }

    public class DirectoryCrawler<T>
    {
        private bool _stopped;

        public DirectoryPath Root { get; }

        public DirectoryCrawledEvent<T> DirectoryCrawled { get; }

        public CrawlOptions Options { get; }

        public DirectoryCrawlStatistics Statistics { get; private set; }

        public DirectoryCrawler
        (
            DirectoryPath root,
            DirectoryCrawledEvent<T> directoryCrawled = null,
            CrawlOptions options = null
        )
        {
            Root = root;
            DirectoryCrawled = directoryCrawled;
            Options = options ?? new CrawlOptions();
            Statistics = new DirectoryCrawlStatistics();
        }

        public virtual void InitCrawl() { }

        public virtual T CrawlComplete() { return default; }

        public T Go()
        {
            Statistics = new DirectoryCrawlStatistics();
            _stopped = false;
            InitCrawl();
            if (!_stopped)
            {
                _Go(Root, 0);
            }
            if (!_stopped)
            {
                return CrawlComplete();
            }
            return default;
        }

        void _Go(DirectoryPath dir, int level)
        {
            // stop recursion
            if (_stopped)
                return;

            // current directory
            var curDirectoryArgs = new DirectoryArgs<T>(dir, level, this, dryRun: Options.DryRun);

            // check dir filter
            if (!(Options.DirectoryFilter?.IsMatch(dir) ?? true))
            {
                var curDirectoryArgsSk = new DirectoryArgs<T>(curDirectoryArgs.Directory, curDirectoryArgs.Level, curDirectoryArgs.DirectoryCrawler, skipReason: SkipReason.UserFiltered, dryRun: curDirectoryArgs.DryRun);
                DirectoryCrawled?.Invoke(DirectoryCrawlEventType.DirectorySkipped, curDirectoryArgsSk, null, stats: Statistics);
                Statistics.DirectoriesSkipped++;
                return;
            }

            // validation
            if (!dir.Exists)
            {
                Stop();
                var curDirectoryArgsEx = new DirectoryArgs<T>(curDirectoryArgs.Directory, curDirectoryArgs.Level, curDirectoryArgs.DirectoryCrawler, dryRun: curDirectoryArgs.DryRun, exception: new ValidationException("Directory does not exist: " + dir));
                DirectoryCrawled?.Invoke(DirectoryCrawlEventType.DirectoryCrawlHalted, curDirectoryArgsEx, null, stats: Statistics);
                return;
            }

            Statistics.DirectoriesCrawled++;

            try
            {
                DirectoryCrawled?.Invoke(DirectoryCrawlEventType.DirectoryEntered, curDirectoryArgs, null, stats: Statistics);
            }
            catch (DirectorySkippedException dsex)
            {
                Statistics.DirectoriesCrawled--;
                Statistics.DirectoriesSkipped++;
                var curDirectoryArgsSk = new DirectoryArgs<T>(curDirectoryArgs.Directory, curDirectoryArgs.Level, curDirectoryArgs.DirectoryCrawler, skipReason: dsex.SkipReason, dryRun: curDirectoryArgs.DryRun);
                DirectoryCrawled?.Invoke(DirectoryCrawlEventType.DirectorySkipped, curDirectoryArgsSk, null, stats: Statistics);
                return;
            }
            catch (DirectoryCrawlHaltedException)
            {
                Stop();
                DirectoryCrawled?.Invoke(DirectoryCrawlEventType.DirectoryCrawlHalted, curDirectoryArgs, null, stats: Statistics);
                return;
            }
            catch (Exception ex)
            {
                Statistics.DirectoriesErrored++;
                if (Options.ReportErrorsAndContinue)
                {
                    var curDirectoryArgsEx = new DirectoryArgs<T>(curDirectoryArgs.Directory, curDirectoryArgs.Level, curDirectoryArgs.DirectoryCrawler, dryRun: curDirectoryArgs.DryRun, exception: ex);
                    DirectoryCrawled?.Invoke(DirectoryCrawlEventType.DirectoryErrored, curDirectoryArgsEx, null, stats: Statistics);
                }
                else throw ex;
            }

            // process files first
            if (!Options.DirectoriesOnly)
            {
                FileArgs<T> fileArgs;
                var files = Options.FileSearchPattern != null
                    ? dir.GetFiles(Options.FileSearchPattern)
                    : dir.GetFiles();

                foreach (var file in files)
                {
                    fileArgs = new FileArgs<T>(file, level + 1, this, dryRun: Options.DryRun);

                    // check file filter
                    if (!(Options.FileFilter?.IsMatch(file) ?? true))
                    {
                        Statistics.FilesSkipped++;
                        var fileArgsSk = new FileArgs<T>(fileArgs.File, fileArgs.Level, this, dryRun: Options.DryRun, skipReason: SkipReason.UserFiltered);
                        DirectoryCrawled?.Invoke(DirectoryCrawlEventType.FileSkipped, curDirectoryArgs, fileArgsSk, stats: Statistics);
                        continue;
                    }

                    Statistics.FilesCrawled++;

                    try
                    {
                        Statistics.SizeOfFilesCrawled += file.Length;
                        DirectoryCrawled?.Invoke(DirectoryCrawlEventType.FileFound, curDirectoryArgs, fileArgs, stats: Statistics);
                    }
                    catch (FileSkippedException fsex)
                    {
                        Statistics.FilesCrawled--;
                        Statistics.SizeOfFilesCrawled -= file.Length;
                        Statistics.FilesSkipped++;
                        var fileArgsEx = new FileArgs<T>(fileArgs.File, fileArgs.Level, this, dryRun: fileArgs.DryRun, skipReason: fsex.SkipReason);
                        DirectoryCrawled?.Invoke(DirectoryCrawlEventType.FileSkipped, curDirectoryArgs, fileArgsEx, stats: Statistics);
                        continue;
                    }
                    catch (DirectoryCrawlHaltedException)
                    {
                        Stop();
                        DirectoryCrawled?.Invoke(DirectoryCrawlEventType.DirectoryCrawlHalted, curDirectoryArgs, fileArgs, stats: Statistics);
                        return;
                    }
                    catch (Exception ex)
                    {
                        if (Options.ReportErrorsAndContinue)
                        {
                            Statistics.FilesErrored++;
                            var fileArgsEx = new FileArgs<T>(fileArgs.File, fileArgs.Level, this, dryRun: fileArgs.DryRun, exception: ex);
                            DirectoryCrawled?.Invoke(DirectoryCrawlEventType.FileErrored, curDirectoryArgs, fileArgsEx, stats: Statistics);
                        }
                        else throw ex;
                    }
                }
            }

            // process subdirectories last
            foreach (var subDir in dir.GetDirectories())
            {
                _Go(subDir, level + 1);
                if (_stopped)
                    return;
            }

            // finally, exit directory
            DirectoryCrawled?.Invoke(DirectoryCrawlEventType.DirectoryExited, curDirectoryArgs, null, stats: Statistics);
        }

        public void Stop()
        {
            _stopped = true;
        }
    }
}
