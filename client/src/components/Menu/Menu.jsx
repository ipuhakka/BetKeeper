import React, { Component } from 'react';
import { connect } from 'react-redux';
import {withRouter} from 'react-router-dom';
import store from '../../store';
import Nav from 'react-bootstrap/Nav';
import PropTypes from 'prop-types';
import Spinner from '../Spinner/Spinner.jsx';
import './Menu.css';

class Menu extends Component{
	render(){

		return(
			<Nav variant="tabs" onSelect={this.handleSelect} as="ul">
				<Nav.Item as="li">
					<Nav.Link eventKey={0} disabled={this.props.disable[0]}>Home</Nav.Link>
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
		const {history} = this.props;

		switch(parseInt(key)){
			case 0:
				history.push('/home');
				break;
			case 1:
				history.push('/bets');
				break;
			case 2:
				history.push('/statistics');
				break;
			case 3:
				history.push('/folders');
				break;
			case 4:
				store.dispatch({type: 'LOGOUT'});
				history.push('/');
				break;
			default:
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

export default connect(mapStateToProps)(withRouter(Menu));
