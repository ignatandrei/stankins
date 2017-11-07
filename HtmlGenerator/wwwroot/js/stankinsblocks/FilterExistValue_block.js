
        Blockly.Blocks['FilterExistValue'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("Filter Exist Value for Column");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Exists value for '), 'fldName');
        this.appendValueInput('valKey') 
        .setCheck('String')
        .appendField('ColumnName:')
        .appendField(new Blockly.FieldTextInput(''), 'fldKey');
        
        this.setTooltip("FilterExistValue");
        this.setHelpUrl("");
        this.setOutput(true, "Filter");
        }
        };
    