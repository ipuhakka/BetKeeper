import React, { Component } from 'react';
import { connect } from 'react-redux';
import {withRouter} from 'react-router-dom';
import store from '../../store';
import Nav from 'react-bootstrap/Nav';
import PropTypes from 'prop-types';
import Spinner from '../Spinner/Spinner.jsx';
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

	render(){
		const { loading, disable } = this.props;
		const {scrollLeft, scrollRight} = this.state;
		document.body.className = loading 
			? 'loading'
			: '';

		return(
			<div ref={this.menuRef} className='menu-div'>
				<button
					onClick={() => this.scrollTo('left')} 
					className={`scroll-button left${scrollLeft
					? ''
					: ' hidden'}`}>{'<'}</button>
				<Nav variant="tabs" onSelect={this.handleSelect} as="ul">
					<Nav.Item as="li">
						<Nav.Link eventKey={0} disabled={disable[0]}>Home</Nav.Link>
					</Nav.Item>
					<Nav.Item as="li">
						<Nav.Link eventKey={1} disabled={disable[1]}>Bets</Nav.Link>
					</Nav.Item>
					<Nav.Item as="li">
						<Nav.Link eventKey={2} disabled={disable[2]}>Statistics</Nav.Link>
					</Nav.Item>
					<Nav.Item as="li">
						<Nav.Link eventKey={3} disabled={disable[3]}>Folders</Nav.Link>
					</Nav.Item>
					<Nav.Item as="li">
						<Nav.Link eventKey={4} disabled={disable[4]}>Logout</Nav.Link>
					</Nav.Item>
					<Spinner as="li" className="spinner" active={loading}/>
				</Nav>
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
	handleOverflow = () => {
		if (_.isNil(this.menuRef.current))
		{
			return;
		}

		const { scrollWidth, clientWidth, scrollLeft } = this.menuRef.current;

		const overflows = scrollWidth - clientWidth;

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
