
        Blockly.Blocks['ReceiverWholeTableSqlServer'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("ReceiverWholeTableSqlServer");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Receiver Table'), 'fldName');
        this.appendValueInput('valtableData') 
        .setCheck('String')
        .appendField('Name Table:')
        .appendField(new Blockly.FieldTextInput(''), 'fldtableData');
        
        this.setTooltip("ReceiverWholeTableSqlServer");
        this.setHelpUrl("");
        this.setOutput(true, "Receiver");
        }
        };
    