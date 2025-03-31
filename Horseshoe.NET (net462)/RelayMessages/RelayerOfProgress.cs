namespace Horseshoe.NET.RelayMessages
{
    /// <summary>
    /// <para>
    /// The base delegate for relaying progress. Use case: code library used by an 
    /// installer UI sends progress updates to the console or a widget e.g. 
    /// progress bar, label, etc.
    /// </para>
    /// <para>
    /// Usage example
    /// </para>
    /// <code>
    /// Relay?.Progress(taskNumber: 0, totalTasks: 5, description: "Initializing...");
    /// // do initialization
    /// Relay?.Progress(taskNumber: 0, totalTasks: 5, description: "1 - Gathering...");
    /// // do task 1
    /// Relay?.Progress(taskNumber: 1, totalTasks: 5, description: "2 - Filtering...");
    /// // do task 2
    /// Relay?.Progress(taskNumber: 2, totalTasks: 5, description: "3 - Configuring...");
    /// // do task 3
    /// Relay?.Progress(description: "4 - Processing (this may take a while)...", 
    ///                 indeterminate: true);
    /// // do task 4
    /// Relay?.Progress(taskNumber: 4, totalTasks: 5, description: "5 - Cleaning up...");
    /// // do task 5
    /// Relay?.Progress(taskNumber: 5, totalTasks: 5, description: "Success!");
    /// // - or, alternatively -
    /// Relay?.Progress(progress: 1.0, description: "Success!");
    /// </code>
    /// </summary>
    /// <param name="taskNumber">
    /// The task number.  Together with <c>totalTasks</c> can be used in lieu of <c>progress</c>. 
    /// 0 (zero) should indicate a pre-run or initializing status or the first task is
    /// incomplete and 1 (one) should mean the first task is complete, etc.
    /// </param>
    /// <param name="totalTasks">The number of tasks whose progress to track.</param>
    /// <param name="progress">
    /// A value between 0.0 and 1.0 (inclusive) indicating a percent complete. Most granular option, 
    /// can be used to 'creep' a.k.a. visually suggest progress during a delayed or long running 
    /// operation such as a slow download or during timeouts and retries.
    /// </param>
    /// <param name="category">The task category or a broad description of the current task, could be used like a title.</param>
    /// <param name="description">A description of the current task.</param>
    /// <param name="indeterminate">If <c>true</c>, can set a UI widget to 'spin' or enter indeterminate mode, default is <c>false</c>.</param>
    public delegate void RelayerOfProgress(int? taskNumber = null, int? totalTasks = null, double? progress = null, string category = null, string description = null, bool indeterminate = false);
}
