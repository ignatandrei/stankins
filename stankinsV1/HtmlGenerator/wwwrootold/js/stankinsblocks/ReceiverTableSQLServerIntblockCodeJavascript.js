
        Blockly.Blocks['ReceiverTableSQLServerInt'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("ReceiverTableSQLServerInt");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput(''), 'fldName');
        this.appendValueInput('valtableData') 
        .setCheck('String')
        .appendField('table name:')
        .appendField(new Blockly.FieldTextInput(''), 'fldtableData');
       
        this.appendValueInput('valLastValue') 
        //.setCheck('String')
        .appendField('LastValue:')
        .appendField(new Blockly.FieldTextInput(''), 'fldLastValue');
        
        this.setTooltip("ReceiverTableSQLServerInt");
        this.setHelpUrl("");
        this.setOutput(true, "Receiver");
        }
        };
    
        Blockly.JavaScript['ReceiverTableSQLServerInt'] = function(block) {
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_tableData = block.getFieldValue('fldtableData');
  var value_tableData = Blockly.JavaScript.valueToCode(block, 'valtableData', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_tableData =value_tableData || "'" + text_tableData + "'";
  
  
  
  var text_LastValue = block.getFieldValue('fldLastValue');
  var value_LastValue = Blockly.JavaScript.valueToCode(block, 'valLastValue', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_LastValue =value_LastValue || "'" + text_LastValue + "'";
  
  var code ='{';
        code+="Name:"+ realValue_Name+",";
        code+="tableData:"+ realValue_tableData+",";        
        code+="LastValue:"+ realValue_LastValue+",";
code+="$type: 'ReceiverDBSqlServer.ReceiverTableSQLServerInt, ReceiverDBSqlServer'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    