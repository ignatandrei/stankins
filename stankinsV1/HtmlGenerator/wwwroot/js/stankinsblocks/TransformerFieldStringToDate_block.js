
        Blockly.Blocks['TransformerFieldStringToDate'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("TransformerFieldStringToDate");
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('String to Date'), 'fldName');
        
		this.appendValueInput('valOldField') 
        .setCheck('String')
        .appendField('OldField:')
        .appendField(new Blockly.FieldTextInput(''), 'fldOldField');
        
        this.appendValueInput('valFormat') 
        .setCheck('String')
        .appendField('Format:')
        .appendField(new Blockly.FieldTextInput(''), 'fldFormat');
        
		this.appendValueInput('valNewField') 
        .setCheck('String')
        .appendField('NewField:')
        .appendField(new Blockly.FieldTextInput(''), 'fldNewField');
        this.setTooltip("TransformerFieldStringToDate");
        this.setHelpUrl("");
        this.setOutput(true, "Transfomer");
        }
        };
    