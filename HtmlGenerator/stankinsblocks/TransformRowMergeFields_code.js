
        Blockly.JavaScript['TransformRowMergeFields'] = function(block) {
  var text_NameField1 = block.getFieldValue('fldNameField1');
  var value_NameField1 = Blockly.JavaScript.valueToCode(block, 'valNameField1', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_NameField1 =value_NameField1 || "'" + text_NameField1 + "'";
  
  var text_NameField2 = block.getFieldValue('fldNameField2');
  var value_NameField2 = Blockly.JavaScript.valueToCode(block, 'valNameField2', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_NameField2 =value_NameField2 || "'" + text_NameField2 + "'";
  
  var text_NameFieldOutput = block.getFieldValue('fldNameFieldOutput');
  var value_NameFieldOutput = Blockly.JavaScript.valueToCode(block, 'valNameFieldOutput', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_NameFieldOutput =value_NameFieldOutput || "'" + text_NameFieldOutput + "'";
  
  var text_Separator = block.getFieldValue('fldSeparator');
  var value_Separator = Blockly.JavaScript.valueToCode(block, 'valSeparator', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Separator =value_Separator || "'" + text_Separator + "'";
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var code ='{';
        code+="NameField1:"+ realValue_NameField1+",";
        code+="NameField2:"+ realValue_NameField2+",";
        code+="NameFieldOutput:"+ realValue_NameFieldOutput+",";
        code+="Separator:"+ realValue_Separator+",";
        code+="Name:"+ realValue_Name+",";
code+="$type: 'Transformers.TransformRowMergeFields, Transformers'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    