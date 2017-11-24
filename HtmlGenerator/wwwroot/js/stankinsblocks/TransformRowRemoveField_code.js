
        Blockly.JavaScript['TransformRowRemoveField'] = function(block) {
  var text_NameFields = block.getFieldValue('fldNameFields');
  var value_NameFields = Blockly.JavaScript.valueToCode(block, 'valNameFields', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_NameFields =value_NameFields || "'" + text_NameFields + "'";
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  
  //debugger;
  var code ='{';
        code+="NameFields:"+ realValue_NameFields+",";
        code+="Name:"+ realValue_Name+",";  
code+="$type: 'Transformers.TransformRowRemoveField, Transformers'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    