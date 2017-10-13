
        Blockly.Blocks['Sender_CSV'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("Sender_CSV");
        
        //this.appendValueInput('valFileMode') 
        //.setCheck('String')
        //.appendField('FileMode:')
        //.appendField(new Blockly.FieldTextInput(''), 'fldFileMode');
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput(''), 'fldName');
        
        this.appendValueInput('valFileName') 
        .setCheck('String')
        .appendField('FileName:')
        .appendField(new Blockly.FieldTextInput(''), 'fldFileName');
        
        
        this.setTooltip("Sender_CSV");
        this.setHelpUrl("");
        this.setOutput(true, "Sender");
        }
        };
    
        Blockly.JavaScript['Sender_CSV'] = function(block) {
  //var text_FileMode = block.getFieldValue('fldFileMode');
  //var value_FileMode = Blockly.JavaScript.valueToCode(block, 'valFileMode', Blockly.JavaScript.ORDER_ATOMIC);
//var realValue_FileMode =value_FileMode || "'" + text_FileMode + "'";
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
 
  
  var text_FileName = block.getFieldValue('fldFileName');
  var value_FileName = Blockly.JavaScript.valueToCode(block, 'valFileName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FileName =value_FileName || "'" + text_FileName + "'";
  
  
  var code ='{';
        //code+="FileMode:"+ realValue_FileMode+",";
        code+="Name:"+ realValue_Name+",";        
        code+="FileName:"+ realValue_FileName+",";
code+="$type: 'SenderToFile.Sender_CSV, SenderToFile'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    