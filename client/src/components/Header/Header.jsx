import React, { Component } from 'react';
import PropTypes from 'prop-types';
import logo from './icon.svg';
import './Header.css';

class Header extends Component{
  render(){
    return(
      <header className="App-header">
				<img src={logo} className="App-logo" alt="logo" />
				<h1 className="App-title">{this.props.title}</h1>
			</header>
    );
  }
}

Header.propTypes = {
  title: PropTypes.string
}

export default Header;
