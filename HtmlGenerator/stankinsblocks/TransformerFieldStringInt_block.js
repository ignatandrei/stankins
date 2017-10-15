
        Blockly.Blocks['TransformerFieldStringInt'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("TransformerFieldStringInt");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('String to int'), 'fldName');
        this.appendValueInput('valOldField') 
        .setCheck('String')
        .appendField('OldField:')
        .appendField(new Blockly.FieldTextInput(''), 'fldOldField');
        this.appendValueInput('valNewField') 
        .setCheck('String')
        .appendField('NewField:')
        .appendField(new Blockly.FieldTextInput(''), 'fldNewField');
        this.setTooltip("TransformerFieldStringInt");
        this.setHelpUrl("");
        this.setOutput(true, "Transformer");
        }
        };
    