import React, { Component } from 'react';
import {withRouter} from 'react-router-dom';
import store from '../../store';
import Navbar from '../Navbar/Navbar';
import PropTypes from 'prop-types';
import _ from 'lodash';

const menuItems = [
	{key: 0, text: 'Home'},
	{key: 1, text: 'Bets'},
	{key: 2, text: 'Statistics'},
	{key: 3, text: 'Folders'},
	{key: 4, text: 'Competitions'},
	{key: 5, text: 'Logout'}
]

/** Application menu */
class Menu extends Component
{
	constructor(props)
	{
		super(props);

		const match = menuItems.find(item => item.text === props.disableValue);

		this.state = {
			disabled: _.isNil(match)
				? null
				: match.key
		}
	}
	
	render()
	{
		return <Navbar 
			items={menuItems}
			handleSelect={this.handleSelect}
			activeKey={this.state.disabled} />;
	}

	handleSelect = async (key) => 
	{
		const {history} = this.props;

		this.setState({
			disabled: key
		});

		history.push('/');
		
		switch(parseInt(key))
		{
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
				history.push('page/competitions');
				break;
			case 5:
				store.dispatch({type: 'LOGOUT'});
				history.push('/');
				break;
			default:
				break;
		}
	}
}

Menu.propTypes = {
  disableValue: PropTypes.string
};

export default withRouter(Menu);
