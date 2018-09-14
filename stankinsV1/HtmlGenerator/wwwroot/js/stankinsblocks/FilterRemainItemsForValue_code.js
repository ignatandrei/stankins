
        Blockly.JavaScript['FilterRemainItemsForValue'] = function(block) {
  var text_Key = block.getFieldValue('fldKey');
  var value_Key = Blockly.JavaScript.valueToCode(block, 'valKey', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Key =value_Key || "'" + text_Key + "'";
  
  var text_ValueSearch = block.getFieldValue('fldValueSearch');
  var value_ValueSearch = Blockly.JavaScript.valueToCode(block, 'valValueSearch', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_ValueSearch =value_ValueSearch || "'" + text_ValueSearch + "'";
 

 var text_Condition = block.getFieldValue('fldCondition');
  var value_Condition = Blockly.JavaScript.valueToCode(block, 'valCondition', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Condition =value_Condition ||  text_Condition ;
 

 
  var text_FilterType = block.getFieldValue('fldFilterType');
  var value_FilterType = Blockly.JavaScript.valueToCode(block, 'valFilterType', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_FilterType =value_FilterType || "'" + text_FilterType + "'";
  
  
  var text_Name = block.getFieldValue('fldName');
  var value_Name = Blockly.JavaScript.valueToCode(block, 'valName', Blockly.JavaScript.ORDER_ATOMIC);
var realValue_Name =value_Name || "'" + text_Name + "'";
  
  var code ='{';
        code+="ColumnName:"+ realValue_Key+",";
        code+="ValueSearch:"+ realValue_ValueSearch+",";
        code+="FilterType:"+ realValue_FilterType+",";
        code+="InvertCondition:"+ realValue_Condition+",";
		
		code+="Name:"+ realValue_Name+",";
		
code+="$type: 'BlocklyClasses.FilterColumnValue, BlocklyClasses'"; ;
code +='}\n';
  return  [code, Blockly.JavaScript.ORDER_NONE];
};;
    