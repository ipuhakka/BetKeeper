import React, { Component} from 'react';
import PropTypes from 'prop-types';
import _ from 'lodash';
import './ScrollableDiv.css';

/**
 * Div to display a scrollable list with shadow. 
 * maximum height should be set for component.
 */
class ScrollableDiv extends Component 
{
    constructor(props)
    {
        super(props);

        this.state = {
            hideBottomShadow: false
        }

        this.scrollRef = React.createRef();
    }

    componentDidUpdate()
    {
      const {scrollHeight, offsetHeight} = this.scrollRef.current;
      
      const isScrollable = scrollHeight > offsetHeight;

      if (isScrollable === this.state.hideBottomShadow && !isScrollable)
      {
        this.setState({hideBottomShadow: true});
      }     
    }

    render()
    {
        const { state, props } = this;

        return <div 
          ref={this.scrollRef}
          className={`scrollableDiv ${state.hideBottomShadow
            ? null
            : 'bottom-shadow'} ${props.className}`}
          onScroll={this.handleScrollChange}>
          {props.children}
        </div>;
    }

  /**
   * Checks if scroll has reached or left the bottom of container.
   * @param {object} e event
   */
  handleScrollChange = (e) => 
  {  
    const { state } = this;
    const scrolledToBottom = this.hasScrollReachedBottom(e);
    const hasScrollBottomStateChanged = !_.isEqual(scrolledToBottom, state.scrolledToBottom);

    if (hasScrollBottomStateChanged)
    {
      this.setState({ hideBottomShadow: scrolledToBottom });
    }
  }

  hasScrollReachedBottom = (e) => 
  {
    const element = e.target;

    return Math.floor(element.scrollHeight - element.scrollTop) <= element.clientHeight;
  }
};

ScrollableDiv.propTypes = {
    className: PropTypes.string
};

export default ScrollableDiv;