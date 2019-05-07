import React, { Component } from 'react';
import { connect } from 'react-redux';
import Nav from 'react-bootstrap/Nav';
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
			<Nav variant="tabs" onSelect={this.handleSelect} as="ul">
				<Nav.Item as="li">
					<Nav.Link className="disabled" eventKey={0} disabled={this.props.disable[0]}>Home</Nav.Link>
				</Nav.Item>
				<Nav.Item as="li">
					<Nav.Link eventKey={1} disabled={this.props.disable[1]}>Bets</Nav.Link>
				</Nav.Item>
				<Nav.Item as="li">
					<Nav.Link eventKey={2} disabled={this.props.disable[2]}>Statistics</Nav.Link>
				</Nav.Item>
				<Nav.Item as="li">
					<Nav.Link eventKey={3} disabled={this.props.disable[3]}>Folders</Nav.Link>
				</Nav.Item>
				<Nav.Item as="li">
					<Nav.Link eventKey={4} disabled={this.props.disable[4]}>Logout</Nav.Link>
				</Nav.Item>
				<Spinner as="li" className="spinner" active={this.props.loading}/>
			</Nav>);
	}

	handleSelect = async (key) => {
		switch(parseInt(key)){
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
				}
				catch (e){
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

Menu.propTypes = {
  disable: PropTypes.array
};

const mapStateToProps = (state, ownProps) => {
  return { ...state.loading};
};

export default connect(mapStateToProps)(Menu);
