﻿namespace NG.Data.Spanner
{
    /// <summary>
	/// Specifies whether to perform synchronous or asynchronous I/O.
	/// </summary>
	public enum IOBehavior
    {
        /// <summary>
        /// Use synchronous I/O.
        /// </summary>
        Synchronous,

        /// <summary>
        /// Use asynchronous I/O.
        /// </summary>
        Asynchronous,
    }
}
