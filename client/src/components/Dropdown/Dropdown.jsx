import React, { Component } from 'react';
import PropTypes from 'prop-types';
import DropdownButton from 'react-bootstrap/lib/DropdownButton';
import MenuItem from 'react-bootstrap/lib/MenuItem';

class Dropdown extends Component{
  constructor(props){
    super(props);

    this.state = {
      selectedKey: this.props.defaultKey === undefined ? 0 : this.props.defaultKey
    };
  };

  render(){
    let i = 0;
    let items = this.props.data.map(item => {
      let menuItem = (<MenuItem active={this.state.selectedKey === i} key={i} onClick={this.handleClick.bind(this, i, this.props.stateKey)}>{this.props.data[i]}</MenuItem>);
      i = i + 1;
      return menuItem;
    });

    return(
      <DropdownButton
        bsStyle="primary"
        title={this.props.title}
        id={this.props.id}>
        {items}
      </DropdownButton>
    );
  }

  handleClick = (key, stateKey) => {
    this.props.onUpdate(key, stateKey);
    this.setState({
      selectedKey: key
    })
  };
};

Dropdown.propTypes = {
  data: PropTypes.array.isRequired,
  onUpdate: PropTypes.func.isRequired,
  title: PropTypes.string.isRequired,
  id: PropTypes.number.isRequired,
  stateKey: PropTypes.string.isRequired,
  defaultKey: PropTypes.number
}

export default Dropdown;
