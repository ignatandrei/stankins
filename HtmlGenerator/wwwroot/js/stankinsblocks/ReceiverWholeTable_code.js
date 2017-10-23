
        Blockly.JavaScript['ReceiverWholeTable'] = function(block) {
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var text_TableName = block.getFieldValue('fldTableName');
  var value_TableName = Blockly.JavaScript.valueToCode(block, 'valTableName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_TableName =value_TableName || "'" + text_TableName + "'";
  
   var text_ConnectionString= block.getFieldValue('fldConnectionString');
  var value_ConnectionString= Blockly.JavaScript.valueToCode(block, 'valConnectionString', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_ConnectionString=value_ConnectionString|| "'" + text_ConnectionString+ "'";
  
  var text_receivertype = block.getFieldValue('ReceiverType');
  var value_receivertype = Blockly.JavaScript.valueToCode(block, 'ReceiverType', Blockly.JavaScript.ORDER_ATOMIC);
  var realValue_receivertype=value_receivertype|| text_receivertype;
  //debugger;
  var code ='{';
		code+="$type: 'BlocklyClasses.ReceiverWholeTable, BlocklyClasses'"; ;
        code+=",Name:"+ realValue_Name;
		code+=",ReceiverType:"+ realValue_receivertype;        		
        code+=",TableName:"+ realValue_TableName;        
		code+=",ConnectionString:"+ realValue_ConnectionString;        
		

code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    