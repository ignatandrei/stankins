
        Blockly.Blocks['TransformRowRemoveField'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("TransformRowRemoveField");
        
        this.appendValueInput('valNameFields') 
        .setCheck('Array')
        .appendField('NameFields:')
        .appendField(new Blockly.FieldTextInput(''), 'fldNameFields');
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput(''), 'fldName');
        
        this.setTooltip("TransformRowRemoveField");
        this.setHelpUrl("");
        this.setOutput(true, "Transformer");
        }
        };
    