
        Blockly.JavaScript['ReceiverWholeTableSqlServer'] = function(block) {
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_tableData = block.getFieldValue('fldtableData');
  var value_tableData = Blockly.JavaScript.valueToCode(block, 'valtableData', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_tableData =value_tableData || "'" + text_tableData + "'";
  
  
  var code ='{';
        code+="Name:"+ realValue_Name+",";
        code+="tableData:"+ realValue_tableData+",";        
code+="$type: 'ReceiverDBSqlServer.ReceiverWholeTableSqlServer, ReceiverDBSqlServer'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    