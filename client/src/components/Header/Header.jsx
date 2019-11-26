import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import Spinner from '../Spinner/Spinner.jsx';
import logo from './icon.svg';
import './Header.css';

class Header extends Component{
  render()
  {
    const {title, loading} = this.props;

		document.body.className = loading 
			? 'loading'
			: '';

    return(
      <header className="App-header">
				<img src={logo} className="App-logo" alt="logo" />
				<h1 className="App-title">{title}</h1>
        <Spinner as="li" className="spinner" active={loading} />
			</header>
    );
  }
}

Header.propTypes = {
  title: PropTypes.string
}

const mapStateToProps = (state) => {
  return { ...state.loading};
};

export default connect(mapStateToProps)(Header);
