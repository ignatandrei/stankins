
        Blockly.Blocks['TransformRowRemoveField'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("Remove Fields");
		
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Remove fields'), 'fldName');
        
        this.appendValueInput('valNameFields') 
        .setCheck('Array')
		.setAlign(Blockly.ALIGN_RIGHT)
        .appendField('name of fields (append list)')
        //.appendField(new Blockly.FieldTextInput(''), 'fldNameFields');
        
        this.setTooltip("TransformRowRemoveField");
        this.setHelpUrl("");
        this.setOutput(true, "Transformer");
        }
        };
    