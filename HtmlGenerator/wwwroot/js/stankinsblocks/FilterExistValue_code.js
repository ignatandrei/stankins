
        Blockly.JavaScript['FilterExistValue'] = function(block) {
  
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_Key = block.getFieldValue('fldKey');
  var value_Key = Blockly.JavaScript.valueToCode(block, 'valKey', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Key =value_Key || "'" + text_Key + "'";
  
  var code ='{';
        code+="Name:"+ realValue_Name+",";
        code+="Key:"+ realValue_Key+",";
code+="$type: 'Transformers.FilterExistValue, Transformers'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    