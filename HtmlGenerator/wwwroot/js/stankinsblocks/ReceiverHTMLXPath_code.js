
        Blockly.JavaScript['ReceiverHTMLXPath'] = function(block) {
  var text_XPaths = block.getFieldValue('fldXPaths');
  var value_XPaths = Blockly.JavaScript.valueToCode(block, 'valXPaths', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_XPaths =value_XPaths || "'" + text_XPaths + "'";
  
  var text_AttributeNames = block.getFieldValue('fldAttributeNames');
  var value_AttributeNames = Blockly.JavaScript.valueToCode(block, 'valAttributeNames', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_AttributeNames =value_AttributeNames || "'" + text_AttributeNames + "'";
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_FileToRead = block.getFieldValue('fldFileToRead');
  var value_FileToRead = Blockly.JavaScript.valueToCode(block, 'valFileToRead', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FileToRead =value_FileToRead || "'" + text_FileToRead + "'";
  
  var code ='{';
        code+="XPaths:"+ realValue_XPaths+",";
        code+="AttributeNames:"+ realValue_AttributeNames+",";
        code+="Name:"+ realValue_Name+",";
        code+="FileToRead:"+ realValue_FileToRead+",";
  code+="$type: 'ReceiverHTML.ReceiverHTMLXPath, ReceiverHTML'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    