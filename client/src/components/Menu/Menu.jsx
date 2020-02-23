import React, { Component } from 'react';
import {withRouter} from 'react-router-dom';
import store from '../../store';
import Navbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';
import PropTypes from 'prop-types';
import './Menu.css';
import _ from 'lodash';

class Menu extends Component{
	constructor(props)
	{
		super(props);

		this.menuRef = React.createRef();

		this.state = {
			scrollLeft: false,
			scrollRight: false
		}
	}

	componentDidMount()
	{
		window.addEventListener('resize', this.handleOverflow);
		this.handleOverflow();
	}

	render()
	{
		const { disableValue } = this.props;
		const {scrollLeft, scrollRight} = this.state;

		const items = [
			{key: 0, value: 'Home'},
			{key: 1, value: 'Bets'},
			{key: 2, value: 'Statistics'},
			{key: 3, value: 'Folders'},
			{key: 4, value: 'Competitions'},
			{key: 5, value: 'Logout'}
		]

		return(
			<div ref={this.menuRef} className='menu-div'>
				<button
					onClick={() => this.scrollTo('left')} 
					className={`scroll-button left${scrollLeft
					? ''
					: ' hidden'}`}>{'<'}</button>
				<Navbar bg='none'>
					<Nav variant="tabs" onSelect={this.handleSelect} as="ul">
						{_.map(items, item =>
							{
								return <Nav.Item key={`nav-item-${item.value}`} as='li'>
									<Nav.Link eventKey={item.key} disabled={disableValue === item.value}>
										{item.value}
									</Nav.Link>
								</Nav.Item>
							})}
					</Nav>
				</Navbar>
				<button
					onClick={() => this.scrollTo('right')}  
					className={`scroll-button right${scrollRight
					? ''
					: ' hidden'}`}>{'>'}</button>
			</div>);
	}

	/**
	 * Scrolls menu to given direction.
	 * @param {string} direction  
	 */
	scrollTo(direction)
	{
		const menu  = this.menuRef.current;

		if (_.isNil(menu))
		{
			return;
		}

		menu.scrollTo({
			left: direction === 'left'
				? 0
				: menu.scrollWidth,
			top: 0,
			behavior: 'smooth'
		});

		this.setState({
			scrollLeft: !this.state.scrollLeft,
			scrollRight: !this.state.scrollRight
		});
	}

	/**
	 * Handles menu overflow.
	 */
	handleOverflow = () => 
	{
		if (_.isNil(this.menuRef.current))
		{			
			return;
		}

		const { scrollWidth, clientWidth, scrollLeft } = this.menuRef.current;

		const overflows = scrollWidth > clientWidth;

		if (overflows)
		{
			if (scrollLeft === 0)
			{
				this.setState(
				{
					scrollRight: true,
					scrollLeft: false
				});
			}
			else 
			{
				this.setState({
					scrollLeft: true,
					scrollRight: false
				});
			}
		}
		else if (this.state.scrollLeft || this.state.scrollRight)
		{
			this.setState({
				scrollLeft: false,
				scrollRight: false
			});
		}
	}

	handleSelect = async (key) => {
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
