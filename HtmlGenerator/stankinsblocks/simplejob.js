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
  var value_receivers = Blockly.JavaScript.valueToCode(block, 'Receivers', Blockly.JavaScript.ORDER_ATOMIC);
  var value_filtersandtransformers = Blockly.JavaScript.valueToCode(block, 'FiltersAndTransformers', Blockly.JavaScript.ORDER_ATOMIC);
  var value_senders = Blockly.JavaScript.valueToCode(block, 'Senders', Blockly.JavaScript.ORDER_ATOMIC);
  debugger;
 //if(!Array.isArray(JSON.parse(value_receivers)))
	value_receivers=[value_receivers];

//if(!Array.isArray(JSON.parse(value_filtersandtransformers)))
	value_filtersandtransformers=[value_filtersandtransformers];
  
//if(!Array.isArray(JSON.parse(value_senders)))
	value_senders=[value_senders];


var code = '\n';
for(var i=0;i<value_receivers.length;i++){
	if(value_receivers[i] == null || value_receivers[i]=='null')
			continue;
	code+=value_receivers[i] + "\n";
}

for(var i=0;i<value_filtersandtransformers.length;i++){
	if(value_filtersandtransformers[i] == null || value_filtersandtransformers[i]=='null')
			continue;
	code+=value_filtersandtransformers[i]+"\n";
}

for(var i=0;i<value_senders.length;i++){
	if(value_senders[i] == null  || value_senders[i]=='null')
			continue;
	code+=value_senders[i]+"\n";
}
  return code;
};

