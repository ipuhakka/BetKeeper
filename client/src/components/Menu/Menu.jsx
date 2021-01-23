import React, { Component } from 'react';
import {withRouter} from 'react-router-dom';
import store from '../../store';
import Navbar from '../Navbar/Navbar';
import PropTypes from 'prop-types';
import _ from 'lodash';

const menuItems = [
	{key: 9, text: 'Home', route: '/page/home'},
	{key: 8, text: 'Bets', route: '/page/bets'},
	{key: 3, text: 'Statistics', route: '/page/statistics'},
	{key: 7, text: 'Folders', route: '/page/folders'},
	{key: 4, text: 'Competitions', route: '/page/competitions'},
	{key: 6, text: 'User settings', route: '/page/usersettings'},
	{key: 5, text: 'Logout', route: '/'}
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

	componentDidUpdate(prevProps)
	{
		if (this.props.location !== prevProps.location)
		{
			const menuItem = menuItems.find(item => item.route === this.props.location.pathname);

			if (!menuItem)
			{
				return;
			}

			this.setState({
				disabled: menuItem.key
			});
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

		const item = menuItems.find(item => item.key === parseInt(key));

		if (item.key === 5)
		{
			store.dispatch({type: 'LOGOUT'});
		}

		history.push('/');
		history.push(item.route);
	}
}

Menu.propTypes = {
  disableValue: PropTypes.string
};

export default withRouter(Menu);
