
        Blockly.JavaScript['TransformAddNewField'] = function(block) {
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_Key = block.getFieldValue('fldKey');
  var value_Key = Blockly.JavaScript.valueToCode(block, 'valKey', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Key =value_Key || "'" + text_Key + "'";
  
  var text_Value = block.getFieldValue('fldValue');
  var value_Value= Blockly.JavaScript.valueToCode(block, 'valValue', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Value=value_Value|| "'" + text_Value + "'";
  
  var value_Increment = Blockly.JavaScript.valueToCode(block, 'valIncrement', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Increment =value_Increment || "'" + text_Increment + "'";

  var code ='{';
        code+="Name:"+ realValue_Name+",";
        code+="Key:"+ realValue_Key+",";
		code+="Value:"+ realValue_Value+",";
		code+="Increment:"+ realValue_Increment+",";

code+="$type: 'Transformers.TransformAddNewField, Transformers'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    