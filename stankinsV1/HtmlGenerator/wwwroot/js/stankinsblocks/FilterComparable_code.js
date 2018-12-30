
        Blockly.JavaScript['FilterComparable'] = function(block) {
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  
  var text_ComparableType = block.getFieldValue('fldComparableType');
  var value_ComparableType = Blockly.JavaScript.valueToCode(block, 'valComparableType', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_ComparableType =value_ComparableType || "'" + text_ComparableType + "'";
  
  var text_Value = block.getFieldValue('fldValue');
  var value_Value = Blockly.JavaScript.valueToCode(block, 'valValue', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Value =value_Value || "'" + text_Value + "'";
  
  var text_FieldName = block.getFieldValue('fldFieldName');
  var value_FieldName = Blockly.JavaScript.valueToCode(block, 'valFieldName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FieldName =value_FieldName || "'" + text_FieldName + "'";
  
  var text_HowToCompareValues = block.getFieldValue('fldHowToCompareValues');
  var value_HowToCompareValues = Blockly.JavaScript.valueToCode(block, 'valHowToCompareValues', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_HowToCompareValues =value_HowToCompareValues || "'" + text_HowToCompareValues + "'";
  
  var code ='{';
        code+="Name:"+ realValue_Name+",";
        code+="ComparableType:"+ realValue_ComparableType+",";
        code+="Value:"+ realValue_Value+",";
        code+="FieldName:"+ realValue_FieldName+",";
        code+="HowToCompareValues:"+ realValue_HowToCompareValues+",";
code+="$type: 'Transformers.FilterComparable, Transformers'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    