namespace StankinsInterfaces
{
    /// <summary>
    /// Interface which defines the members to be implemented by transformes in order to apply filters. It inherits the method Run from IFilterTransformer.
    /// </summary>
    public interface ITransform: IFilterTransformer
    {
        /// <summary>
        /// Input data.
        /// </summary>
        IRow[] valuesRead { get; set; }

        /// <summary>
        /// Output data. It contains rows after filter was applied (after the method Run was executed).
        /// </summary>
        IRow[] valuesTransformed{ get; set; }
    }
}