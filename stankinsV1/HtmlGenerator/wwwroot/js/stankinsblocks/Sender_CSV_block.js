
        Blockly.Blocks['Sender_CSV'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("Sender_CSV");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Sender to CSV'), 'fldName');
       
        this.appendValueInput('valFileName') 
        .setCheck('String')
        .appendField('FileName:')
        .appendField(new Blockly.FieldTextInput(''), 'fldFileName');
		
        this.appendValueInput('valFileMode') 
        .setCheck('Number')
        .appendField('FileMode:')
            .appendField(new Blockly.FieldDropdown([
                ["CreateNew", "1"],
                ["Create", "2"],
                ["Open", "3"],
                ["OpenOrCreate", "4"],
                ["Truncate", "5"],
                ["Append", "6"]

            ]), 'fldFileMode');
        //.appendField(new Blockly.FieldTextInput(''), 'fldFileMode')
        ;
        this.setTooltip("Sender_CSV");
        this.setHelpUrl("");
        this.setOutput(true, "Sender");
        }
        };
    