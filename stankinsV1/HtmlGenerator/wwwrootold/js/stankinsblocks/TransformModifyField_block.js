
        Blockly.Blocks['TransformModifyField'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("TransformModifyField");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Modify Field'), 'fldName');
        this.appendValueInput('valKey') 
        .setCheck('String')
        .appendField('Field Name:')
        .appendField(new Blockly.FieldTextInput(''), 'fldKey');
        this.appendValueInput('valFormatString') 
        .setCheck('String')
        .appendField('FormatString:')
        .appendField(new Blockly.FieldTextInput('{0}'), 'fldFormatString');
        
        this.setTooltip("TransformModifyField");
        this.setHelpUrl("");
        this.setOutput(true, "Transformer");
        }
        };
    