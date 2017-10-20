
        Blockly.Blocks['ReceiverWholeTable'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("ReceiverWholeTable");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Receiver Table'), 'fldName');
        this.appendValueInput('valTableName') 
        .setCheck('String')
        .appendField('Name Table:')
        .appendField(new Blockly.FieldTextInput(''), 'fldTableName');
        
		 this.appendValueInput('valConnectionString') 
        .setCheck('String')
        .appendField('Connection String:')
        .appendField(new Blockly.FieldTextInput(''), 'fldConnectionString');
        
		
        this.setTooltip("ReceiverWholeTable");
        this.setHelpUrl("");
        this.setOutput(true, "Receiver");
        }
        };
    