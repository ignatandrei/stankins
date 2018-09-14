using System;

namespace StankinsInterfaces
{
    public interface IFilterTransformTo<T>: IFilterTransformer
    {
        IRow[] valuesToBeSent { set; }
        T Result { get; }
    }
    public interface IFilterTransformToString: IFilterTransformTo<string>
    {
        
    }
    public interface IFilterTransformToDateTime : IFilterTransformTo<DateTime>
    {
        
    }
}
