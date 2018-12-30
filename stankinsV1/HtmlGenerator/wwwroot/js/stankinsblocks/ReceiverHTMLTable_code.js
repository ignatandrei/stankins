
        Blockly.JavaScript['ReceiverHTMLTable'] = function(block) {
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_FileToRead = block.getFieldValue('fldFileToRead');
  var value_FileToRead = Blockly.JavaScript.valueToCode(block, 'valFileToRead', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FileToRead =value_FileToRead || "'" + text_FileToRead + "'";
  
  
  var text_LastValue = block.getFieldValue('fldLastValue');
  var value_LastValue = Blockly.JavaScript.valueToCode(block, 'valLastValue', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_LastValue =value_LastValue || "'" + text_LastValue + "'";
  
  
  var code ='{';
        code+="Name:"+ realValue_Name+",";
        code+="FileToRead:"+ realValue_FileToRead+",";
        
        
code+="$type: 'ReceiverFile.ReceiverHTMLTable, ReceiverHTML'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    