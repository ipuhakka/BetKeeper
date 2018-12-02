import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import Nav from 'react-bootstrap/lib/Nav';
import NavItem from 'react-bootstrap/lib/NavItem';
import PropTypes from 'prop-types';
import App from '../../App.jsx';
import Bets from '../../views/Bets/Bets.jsx';
import Folders from '../../views/Folders/Folders.jsx';
import Home from '../../views/Home/Home.jsx';
import Statistics from '../../views/Statistics/Statistics.jsx';

class Menu extends Component{
	render(){
		return(
			<Nav bsStyle="tabs" onSelect={this.handleSelect}>
				<NavItem eventKey={0} disabled={this.props.disable[0]}>Home</NavItem>
				<NavItem eventKey={1} disabled={this.props.disable[1]}>Bets</NavItem>
				<NavItem eventKey={2} disabled={this.props.disable[2]}>Statistics</NavItem>
				<NavItem eventKey={3} disabled={this.props.disable[3]}>Folders</NavItem>
				<NavItem eventKey={4} disabled={this.props.disable[4]}>Logout</NavItem>
			</Nav>);
	}

	handleSelect = (key) => {
		switch(key){
			case 0:
				ReactDOM.render(<Home />, document.getElementById('root'));
				break;
			case 1:
				ReactDOM.render(<Bets/>, document.getElementById('root'));
				break;
			case 2:
				ReactDOM.render(<Statistics />, document.getElementById('root'));
				break;
			case 3:
				ReactDOM.render(<Folders />, document.getElementById('root'));
				break;
			case 4:
				window.sessionStorage.setItem('loggedUser', null);
				window.sessionStorage.setItem('token', null);
				ReactDOM.render(<App />, document.getElementById('root'));
				break;
			default:
				console.log("clicked item " + key);
				break;
		}
	}
}

Menu.propTypes = {
           disable: PropTypes.array
};

export default Menu;
