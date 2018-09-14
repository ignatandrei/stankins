namespace Transformers
{
    public class TransformerFieldIntString:TransformOneValueGeneral
    {
        public TransformerFieldIntString(string oldField, string newField):base("(oldValue??0).ToString()", oldField,newField)
        {

        }
    }
}
