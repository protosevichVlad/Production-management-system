Vue.component('universal-item-vue-js', {
  props: ['item'],
  template: `
    <div class="bx--row">
      <div class="bx--col-md-1">
        <img :src="alsoTask.item.imagePath" alt="" class="altiumdb--entity-table--image"/>
      </div>
      <div class="bx--col-md-4">
        <div class="bx--grid">
          <div class="bx--row">{{item.name}}</div>
          <div class="bx--row" style="height: 4px"></div>
          <div class="bx--row">{{item.partNumber}}</div>
          <div class="bx--row" style="height: 4px"></div>
          <div class="bx--row">{{item.category}}</div>
        </div>
      </div>
      <div class="bx--col-md-3">
        {{item.description}}
      </div>
    </div>`
});