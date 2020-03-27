using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CameraTransparent : MonoBehaviour
{
    // 被写体
    [SerializeField]
    private Transform subject_;
    // 遮蔽物のリスト
    [SerializeField]
    private List<string> coverLayerNameList_;
    // 遮蔽物とするレイヤーマスク
    private int layerMask_;
    // 現在の検出した遮蔽物のRendererコンポーネント
    public List<Renderer> rendererHitsList_ = new List<Renderer>();
    // ひとつ前に検出した遮蔽物のRendererコンポーネント
    public Renderer[] rendererHitsPrevs_;


    void Start()
    {
        layerMask_ = 0;
        foreach (string _layerName in coverLayerNameList_)
        {
            layerMask_ |= 1 << LayerMask.NameToLayer(_layerName);
        }
    }

    void Update()
    {
        // カメラと被写体を結ぶrayを作成
        Vector3 _difference = (subject_.transform.position - this.transform.position);
        Vector3 _direction = _difference.normalized;
        Ray _ray = new Ray(this.transform.position, _direction);

        // 前回の結果を退避してからRaycastして今回の遮蔽物のリストを取得する
        RaycastHit[] _hits = Physics.RaycastAll(_ray, _difference.magnitude, layerMask_);


        rendererHitsPrevs_ = rendererHitsList_.ToArray();
        rendererHitsList_.Clear();
        // 遮蔽物は透明にする
        foreach (RaycastHit _hit in _hits)
        {
            // 遮蔽物が被写体の場合は例外とする
            if (_hit.collider.gameObject == subject_)
            {
                continue;
            }

            // 遮蔽物の透明度を変える
            Renderer _renderer = _hit.collider.gameObject.GetComponent<Renderer>();
            if (_renderer != null)
            {
                rendererHitsList_.Add(_renderer);
                Color objColor = _renderer.material.color;
                _renderer.material.color = new Color(objColor.r, objColor.g, objColor.b, 0.3f);
            }
        }

        // 前回まで対象で，今回対象でなくなったものは表示を元に戻す。
        foreach (Renderer _renderer in rendererHitsPrevs_.Except<Renderer>(rendererHitsList_))
        {
            // 遮蔽物の透明度を元に戻す
            if (_renderer != null)
            {
                Color objColor = _renderer.material.color;
                _renderer.material.color = new Color(objColor.r, objColor.g, objColor.b, 1.0f);
            }
        }
    }
}
