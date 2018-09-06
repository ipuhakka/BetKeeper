import React, { Component } from 'react';
import FormControl from 'react-bootstrap/lib/FormControl';
import PropTypes from 'prop-types';

class MaskedInput extends Component{
	constructor(props){
		super(props);
		
		this.state = {
			value: "",
			hiddenText: ""
		};
		
		this.setMaskedText = this.setMaskedText.bind(this);
		this.isNotCapslock = this.isNotCapslock.bind(this);
		this.maskPassword = this.maskPassword.bind(this);
	}
	
	render(){		
		return (
			<FormControl
				type={this.props.type}
				value={this.state.hiddenText}
				placeholder={this.props.placeholder}
				onKeyDown={this.setMaskedText}
				className={this.props.className}
			/>
		);
	}
	
	setMaskedText(e) {
		var content = '';
		var value = this.state.value;
		if (e.keyCode > 32 && e.keyCode < 126) {
			//find out if caps was on
			if (this.isNotCapslock(e)) {
				value = value + String.fromCharCode(e.keyCode + 32);
			}
			else
				value = value + String.fromCharCode(e.keyCode);

		}

		if (e.keyCode === 8) {
			//cut the last char from password and textarea
			value = value.substr(0, value.length - 1);
		}
		
		//fill the input box with char *    
		content = this.maskPassword(value);
		this.setState({
			hiddenText: content
		});
		
		this.setState({
			value: value
		});
		this.props.onChange(value);
	}
	
	isNotCapslock(e){

		e = (e) ? e : window.event;

		var charCode = false;
		if (e.which) {
			charCode = e.which;
		} else if (e.keyCode) {
			charCode = e.keyCode;
		}

		var shifton = false;
		if (e.shiftKey) {
			shifton = e.shiftKey;
		} else if (e.modifiers) {
			shifton = !!(e.modifiers & 4);
		}

		if (charCode >= 97 && charCode <= 122 && shifton) {
			return true;
		}

		if (charCode >= 65 && charCode <= 90 && !shifton) {
			return true;
		}

		return false;
    }

	maskPassword(wordToMask) {
    //helper function to do the actual masking of textarea

		var masked = '';
		for (var i = 0; i < wordToMask.length; i++) {
			masked = masked + '*';
		}
		return masked;
	}
}

MaskedInput.propTypes = {
           onChange: PropTypes.func.isRequired
     };	

export default MaskedInput;