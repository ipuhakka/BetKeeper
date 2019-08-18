import React, { Component } from 'react';
import PropTypes from 'prop-types';
import DropdownButton from 'react-bootstrap/DropdownButton';
import DropdownItem from 'react-bootstrap/DropdownItem';

class Dropdown extends Component{
  constructor(props){
    super(props);

    this.state = {
      selectedKey: this.props.defaultKey === undefined ? 0 : this.props.defaultKey
    };
  };

  render(){
    const { props } = this;

    let i = 0;
    let items = props.data.map(item => {
      let menuItem = (<DropdownItem active={this.state.selectedKey === i} key={i}
        onClick={this.handleClick.bind(this, i, props.stateKey)}>{props.data[i]}</DropdownItem>);
      i = i + 1;
      return menuItem;
    });

    return(
      <DropdownButton
        variant="primary"
        title={props.title}
        id={props.id}
        {...props}>
        {items}
      </DropdownButton>
    );
  }

  handleClick = (key, stateKey) => {
    this.props.onUpdate(key, stateKey);
    this.setState({
      selectedKey: key
    });
  };
};

Dropdown.propTypes = {
  data: PropTypes.array.isRequired,
  onUpdate: PropTypes.func.isRequired,
  title: PropTypes.string.isRequired,
  id: PropTypes.number.isRequired,
  stateKey: PropTypes.string.isRequired,
  defaultKey: PropTypes.number,
  className: PropTypes.string
}

export default Dropdown;
