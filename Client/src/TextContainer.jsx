import React, { Component } from 'react';
import './css/App.css';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';

class TextContainer extends Component{
	render(){
		var items = this.props.items.map(function(item, i){
			return (<ListGroupItem key={i}>{item}</ListGroupItem>);
		});
		
		return(
			<div className={this.props.className}>
				<ListGroup>{items}</ListGroup>
			</div>
		);
	}
}

export default TextContainer;