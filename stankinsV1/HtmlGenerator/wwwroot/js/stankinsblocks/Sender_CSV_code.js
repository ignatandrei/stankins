
        Blockly.JavaScript['Sender_CSV'] = function(block) {
  var text_FileMode = block.getFieldValue('fldFileMode');
  var value_FileMode = Blockly.JavaScript.valueToCode(block, 'valFileMode', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FileMode =value_FileMode || "'" + text_FileMode + "'";
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_FileName = block.getFieldValue('fldFileName');
  var value_FileName = Blockly.JavaScript.valueToCode(block, 'valFileName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FileName =value_FileName || "'" + text_FileName + "'";
  
  
  var code ='{';
  code+="$type: 'SenderToFile.Sender_CSV, SenderToFile'"; ;
        code+=",Name:"+ realValue_Name;
        code+=",FileMode:"+ realValue_FileMode;        
        code+=",FileName:"+ realValue_FileName;
        

code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    