Blockly.Blocks['simplejob'] = {
  init: function() {
    this.appendDummyInput()
        .appendField("Receivers");
    this.appendValueInput("Receivers")
        .setCheck(["Receiver", "Array"]);
    this.appendDummyInput()
        .appendField("Filters And Transformers");
    this.appendValueInput("FiltersAndTransformers")
        .setCheck(["Filter", "Array", "Transformer"]);
    this.appendDummyInput()
        .appendField("Senders");
    this.appendValueInput("Senders")
        .setCheck(["Sender", "Array"]);
    this.setInputsInline(false);
    this.setColour(230);
 this.setTooltip("");
 this.setHelpUrl("");
  }
};
Blockly.JavaScript['simplejob'] = function(block) {
  var value_receivers = (Blockly.JavaScript.valueToCode(block, 'Receivers', Blockly.JavaScript.ORDER_ATOMIC));
  var value_filtersandtransformers = (Blockly.JavaScript.valueToCode(block, 'FiltersAndTransformers', Blockly.JavaScript.ORDER_ATOMIC));
  var value_senders = (Blockly.JavaScript.valueToCode(block, 'Senders', Blockly.JavaScript.ORDER_ATOMIC));
  //debugger;
 if(!Array.isArray((value_receivers)))
	value_receivers=[value_receivers];

if(!Array.isArray((value_filtersandtransformers)))
	value_filtersandtransformers=[value_filtersandtransformers];
  
if(!Array.isArray((value_senders)))
	value_senders=[value_senders];


var code = '\n';
code+="var job={";
code += '\n';
code += "'$type': 'StanskinsImplementation.SimpleJob, StanskinsImplementation',";
code += '\n';

code += "'Receivers': {";
code += '\n';

code += "'$type': 'StankinsInterfaces.OrderedList`1[[StankinsInterfaces.IReceive, StankinsInterfaces]], StankinsInterfaces'";
code += '\n';

var nr=0;
for(var i=0;i<value_receivers.length;i++){
	var valLoop=value_receivers[i];
	if(valLoop == null || valLoop=='null')
			continue;
	code += ",'"+ nr++  + "': ";
	code += '\n';
	code+=(valLoop) + "\n";
	code += "";
	code += '\n';
}
code += "},";//end of receivers
code += '\n';

code += '\n';
code += "'FiltersAndTransformers': {";
code += '\n';
code += "'$type': 'StankinsInterfaces.OrderedList`1[[StankinsInterfaces.IFilterTransformer, StankinsInterfaces]], StankinsInterfaces'";
code += '\n';

nr=0;
for(var i=0;i<value_filtersandtransformers.length;i++){
	var valLoop=value_filtersandtransformers[i];
	if(valLoop == null || valLoop=='null')
			continue;

	code += ",'"+ nr++  + "': ";
	code += '\n';
	code+=(valLoop) + "\n";
	code += "";
	code += '\n';

	
}

code += "},";//end of filters transformers
code += '\n';

code += "'Senders': {";
code += '\n';
code += "'$type': 'StankinsInterfaces.OrderedList`1[[StankinsInterfaces.ISend, StankinsInterfaces]], StankinsInterfaces'";
code += '\n';	
	nr=0;
for(var i=0;i<value_senders.length;i++){
	var valLoop=value_senders[i];
	if(valLoop == null  || valLoop=='null')
			continue;
	code += ",'"+ nr++  + "': ";
	code += '\n';
	code+=(valLoop) + "\n";
	code += "";
	code += '\n';

}
code += "}";//end of senders
code += '\n';

code += "}";//end of job
code += '\n';
code += ';//var t=eval(JSON.stringify(job));';
code += '\n';
code += 'job;\n';
  return code;
};

