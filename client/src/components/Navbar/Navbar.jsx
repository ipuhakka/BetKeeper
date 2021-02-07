import React, { Component } from 'react';
import RBNavbar from 'react-bootstrap/Navbar';
import Nav from 'react-bootstrap/Nav';
import PropTypes from 'prop-types';
import './Navbar.css';
import _ from 'lodash';

/** Navbar component with overflow handling */
class Navbar extends Component 
{
    constructor(props)
    {
        super(props);

        this.menuRef = React.createRef();

        this.state = {
            selectedKey: null,
            scrollLeft: false,
			scrollRight: false
        }

        this.handleOverflow = this.handleOverflow.bind(this);
        this.handleSelect = this.handleSelect.bind(this);
    }
    
	componentDidMount()
	{
		window.addEventListener('resize', this.handleOverflow);
		this.handleOverflow();
	}

    /**
	 * Scrolls menu to given direction.
	 * @param {string} direction  
	 */
	scrollTo(direction)
	{
		const menu = this.menuRef.current;

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
	handleOverflow()
	{
		if (_.get(this, 'menuRef.current', null) === null)
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
    
    /** Change event  */
    handleSelect(key)
    {
        this.setState({ selectedKey: key });

        this.props.handleSelect(key);
    }

    render()
    {
        const { items, activeKey } = this.props;
        const {scrollLeft, scrollRight, selectedKey} = this.state;
        
        return(
			<div ref={this.menuRef} className='menu-div'>
				<button
					onClick={() => this.scrollTo('left')} 
					className={`scroll-button left${scrollLeft
					? ''
					: ' hidden'}`}>{'<'}</button>
				<RBNavbar bg='none'>
					<Nav variant="tabs" onSelect={this.handleSelect} as="ul">
						{_.map(items, item =>
							{
								const active = item.key === selectedKey || item.key === activeKey;
								return <Nav.Item key={`nav-item-${item.text}`} as='li'>
									<Nav.Link eventKey={item.key} active={active}>
										{item.text}
									</Nav.Link>
								</Nav.Item>
							})}
					</Nav>
				</RBNavbar>
				<button
					onClick={() => this.scrollTo('right')}  
					className={`scroll-button right${scrollRight
					? ''
					: ' hidden'}`}>{'>'}</button>
			</div>);
    }
};

Navbar.propTypes = {
    items: PropTypes.arrayOf(
        PropTypes.shape({
            key: PropTypes.oneOfType([
                PropTypes.number,
                PropTypes.string
            ]).isRequired,
            text: PropTypes.string.isRequired
        })
    ),
    handleSelect: PropTypes.func.isRequired,
    activeKey: PropTypes.oneOfType([
        PropTypes.string,
        PropTypes.number])
}

export default Navbar;