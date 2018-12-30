
        Blockly.JavaScript['ReceiverCSVWholeFile'] = function(block) {
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_FileToRead = block.getFieldValue('fldFileToRead');
  var value_FileToRead = Blockly.JavaScript.valueToCode(block, 'valFileToRead', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FileToRead =value_FileToRead || "'" + text_FileToRead + "'";
  
  var text_FileEnconding = block.getFieldValue('fldFileEnconding');
  var value_FileEnconding = Blockly.JavaScript.valueToCode(block, 'valFileEnconding', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FileEnconding =value_FileEnconding || "'" + text_FileEnconding + "'";
  
  var text_ReadAllFirstTime = block.getFieldValue('fldReadAllFirstTime');
  var value_ReadAllFirstTime = Blockly.JavaScript.valueToCode(block, 'valReadAllFirstTime', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_ReadAllFirstTime =value_ReadAllFirstTime || "'" + text_ReadAllFirstTime + "'";
  
  
  var code ='{';
        code+="Name:"+ realValue_Name+",";
        code+="FileToRead:"+ realValue_FileToRead+",";
        code+="FileEnconding:"+ realValue_FileEnconding+",";
        code+="ReadAllFirstTime:"+ realValue_ReadAllFirstTime+",";
code+="$type: 'ReceiverCSV.ReceiverCSVWholeFile, ReceiverCSV'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    