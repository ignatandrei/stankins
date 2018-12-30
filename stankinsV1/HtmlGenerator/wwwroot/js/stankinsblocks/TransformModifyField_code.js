
        Blockly.JavaScript['TransformModifyField'] = function(block) {
  
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_Key = block.getFieldValue('fldKey');
  var value_Key = Blockly.JavaScript.valueToCode(block, 'valKey', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Key =value_Key || "'" + text_Key + "'";
  
  var text_FormatString = block.getFieldValue('fldFormatString');
  var value_FormatString = Blockly.JavaScript.valueToCode(block, 'valFormatString', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FormatString =value_FormatString || "'" + text_FormatString + "'";
  
  var code ='{';
        code+="Name:"+ realValue_Name+",";
        code+="Key:"+ realValue_Key+",";
        code+="FormatString:"+ realValue_FormatString+",";
code+="$type: 'Transformers.TransformModifyField, Transformers'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    