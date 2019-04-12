import React, { Component } from 'react';
import Nav from 'react-bootstrap/lib/Nav';
import NavItem from 'react-bootstrap/lib/NavItem';
import PropTypes from 'prop-types';
import App from '../../App.jsx';
import Bets from '../../views/Bets/Bets.jsx';
import Folders from '../../views/Folders/Folders.jsx';
import Home from '../../views/Home/Home.jsx';
import Statistics from '../../views/Statistics/Statistics.jsx';
import Spinner from '../Spinner/Spinner.jsx';
import {changeToComponent} from '../../changeView';
import {deleteToken} from '../../js/Requests/Token';
import './Menu.css';

class Menu extends Component{
	render(){
		return(
			<Nav bsStyle="tabs" onSelect={this.handleSelect}>
				<NavItem eventKey={0} disabled={this.props.disable[0]}>Home</NavItem>
				<NavItem eventKey={1} disabled={this.props.disable[1]}>Bets</NavItem>
				<NavItem eventKey={2} disabled={this.props.disable[2]}>Statistics</NavItem>
				<NavItem eventKey={3} disabled={this.props.disable[3]}>Folders</NavItem>
				<NavItem eventKey={4} disabled={this.props.disable[4]}>Logout</NavItem>
				<Spinner className="spinner" active={this.props.loading}/>
			</Nav>);
	}

	handleSelect = async (key) => {
		switch(key){
			case 0:
				changeToComponent(<Home/>);
				break;
			case 1:
				changeToComponent(<Bets/>);
				break;
			case 2:
				changeToComponent(<Statistics/>);
				break;
			case 3:
				changeToComponent(<Folders/>);
				break;
			case 4:
				try {
					await deleteToken();
				} catch (e){
					console.log("error in deleteToken");
				}
				window.sessionStorage.setItem('loggedUser', null);
				window.sessionStorage.setItem('token', null);
				window.sessionStorage.setItem('loggedUserID', -1);
				changeToComponent(<App/>);
				break;
			default:
				console.log("clicked item " + key);
				break;
		}
	}
}

Menu.defaultProps = {
	loading: false
};

Menu.propTypes = {
  disable: PropTypes.array,
	loading: PropTypes.bool
};

export default Menu;
