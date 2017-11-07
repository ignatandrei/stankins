
        Blockly.Blocks['FilterRemainItemsForValue'] = {
        init: function() {
        this.appendDummyInput()
        .appendField("Filter Values search ");
        this.appendValueInput('valName') 
        .setCheck('String')
        .appendField('Name:')
        .appendField(new Blockly.FieldTextInput('Filter column for value'), 'fldName');
        
        this.appendValueInput('valKey') 
        .setCheck('String')
        .appendField('Column:')
        .appendField(new Blockly.FieldTextInput(''), 'fldKey');
       this.appendValueInput('valFilterType') 
        .setCheck('String')
        .appendField('FilterType:')
        
		 .appendField(new Blockly.FieldDropdown([
                ["Contains", "1"],
                ["StartsWith", "2"],
                ["Endswith", "3"],
                ["Equal", "4"]
                
            ]), 'fldFilterType');
		this.appendValueInput('valValueSearch') 
        .setCheck('String')
        .appendField('ValueSearch:')
        .appendField(new Blockly.FieldTextInput(''), 'fldValueSearch');
        
        
        this.setTooltip("FilterRemainItemsForValue");
        this.setHelpUrl("");
        this.setOutput(true, "Filter");
        }
        };
    