
        Blockly.JavaScript['TransformKeyValue'] = function(block) {
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var code ='{';
        code+="Name:"+ realValue_Name+",";
code+="$type: 'Transformers.TransformKeyValue, Transformers'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    