import React, { Component } from 'react';
import PropTypes from 'prop-types';
import DropdownButton from 'react-bootstrap/DropdownButton';
import DropdownItem from 'react-bootstrap/DropdownItem';
import './Dropdown.css';

class Dropdown extends Component
{
  constructor(props)
  {
    super(props);

    this.state = {
      selectedKey: this.props.defaultKey === undefined ? 0 : this.props.defaultKey
    };
  };

  render()
  {
    const { props, state } = this;

    let i = 0;
    let items = props.data.map(item => 
    {
      let menuItem = (<DropdownItem active={state.selectedKey === i} key={i}
        onClick={this.handleClick.bind(this, i, props.stateKey)}>{props.data[i]}</DropdownItem>);
      i = i + 1;
      return menuItem;
    });

    const title = props.selectedItemAsTitle
      ? props.data[state.selectedKey]
      : props.title;

    return(
      <DropdownButton
        className={`${props.className} dropdownButton`}
        variant="primary"
        title={title}
        id={props.id}>
        {items}
      </DropdownButton>
    );
  }

  handleClick = (index, stateKey) => 
  {
    this.props.onUpdate(index, stateKey);
    this.setState({
      selectedKey: index
    });
  };
};

Dropdown.propTypes = {
  data: PropTypes.array.isRequired,

  /** Update function called.
   * Called with index of selection and
   * optionally given stateKey.
   */
  onUpdate: PropTypes.func.isRequired,
  title: PropTypes.string.isRequired,

  selectedItemAsTitle: PropTypes.bool,

  id: PropTypes.oneOfType([
    PropTypes.number,
    PropTypes.string]).isRequired,

  /** Key which is returned in update call */
  stateKey: PropTypes.string,

  /** Default selected index */
  defaultKey: PropTypes.number,
  className: PropTypes.string
}

export default Dropdown;
