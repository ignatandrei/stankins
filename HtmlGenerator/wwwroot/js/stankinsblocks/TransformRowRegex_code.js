
        Blockly.JavaScript['TransformRowRegex'] = function(block) {
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_RegexWithGroups = block.getFieldValue('fldRegexWithGroups');
  var value_RegexWithGroups = Blockly.JavaScript.valueToCode(block, 'valRegexWithGroups', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_RegexWithGroups =value_RegexWithGroups || "'" + text_RegexWithGroups + "'";
  
  var text_Key = block.getFieldValue('fldKey');
  var value_Key = Blockly.JavaScript.valueToCode(block, 'valKey', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Key =value_Key || "'" + text_Key + "'";
  
  var code ='{';
        code+="Name:"+ realValue_Name+",";
        code+="RegexWithGroups:"+ realValue_RegexWithGroups+",";
        code+="Key:"+ realValue_Key+",";
code+="$type: 'Transformers.TransformRowRegex, Transformers'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    