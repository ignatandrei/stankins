
        Blockly.Blocks['TransformAddNewField'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("TransformAddNewField");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Add field'), 'fldName');
        this.appendValueInput('valKey') 
        .setCheck('String')
        .appendField('Column:')
        .appendField(new Blockly.FieldTextInput(''), 'fldKey');
        
		this.appendValueInput('valValue') 
        .setCheck('String')
        .appendField('Value:')
        .appendField(new Blockly.FieldTextInput(''), 'fldValue');
        
        this.setTooltip("TransformAddNewField");
        this.setHelpUrl("");
        this.setOutput(true, "Transformer");
        }
        };
    