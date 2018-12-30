
        Blockly.Blocks['ReceiverCSVWholeFile'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("ReceiverCSVWholeFile");
        
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Read CSV'), 'fldName');
        this.appendValueInput('valFileToRead') 
        .setCheck('String')
        .appendField('FileToRead:')
        .appendField(new Blockly.FieldTextInput(''), 'fldFileToRead');
        this.appendValueInput('valFileEnconding') 
        .setCheck('String')
        .appendField('FileEnconding:')
        .appendField(new Blockly.FieldDropdown([
                ["utf-8", 'utf-8'],
                ["ascii", 'ascii'],
                
            ]), 'fldFileEnconding');
		
		
        this.appendValueInput('valReadAllFirstTime') 
        .setCheck('Boolean')
        .appendField('ReadAllFirstTime:')
        .appendField(new Blockly.FieldDropdown([
                ["ReadAll", 'true'],
                ["ReadLine", 'false'],
                
            ]), 'fldReadAllFirstTime');
        
        this.setTooltip("ReceiverCSVWholeFile");
        this.setHelpUrl("");
        this.setOutput(true, "Receiver");
        }
        };
    